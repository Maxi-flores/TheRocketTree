import * as functions from "firebase-functions/v1";
import * as fs from "fs";
import * as path from "path";

import { firestore } from "./firebase";
import {
  applyTaskCompletion,
  applyReflection
} from "./progressionInterpreter";

/**
 * ─────────────────────────────────────────────
 * Initialization
 * ─────────────────────────────────────────────
 * NOTE:
 * firebase-admin is initialized exactly once
 * inside firebase.ts. Do NOT initialize here.
 */
const db = firestore;

/**
 * ─────────────────────────────────────────────
 * AUTH BOOTSTRAP FUNCTION (PRODUCTION SAFE)
 * ─────────────────────────────────────────────
 */
export const onAuthUserCreate = functions.auth.user().onCreate(async (user) => {
  const uid = user.uid;
  const now = new Date().toISOString();

  const userRef = db.collection("users").doc(uid);
  const treeRef = db.collection("treeState").doc(uid);

  let writes = 0;
  const batch = db.batch();

  const userSnap = await userRef.get();
  if (!userSnap.exists) {
    batch.set(userRef, {
      userId: uid,
      createdAt: now,
      lastActiveAt: now,
      timezone: "UTC",
      locale: "en-US",
      accountState: "active",
      subscriptionTier: "free",
      flags: { hasCompletedOnboarding: false }
    });
    writes++;
  }

  const treeSnap = await treeRef.get();
  if (!treeSnap.exists) {
    batch.set(treeRef, {
      userId: uid,
      mass: 1.0,
      structure: 0.5,
      vitality: 0.8,
      lastUpdatedAt: now
    });
    writes++;
  }

  if (writes > 0) {
    await batch.commit();
  }

  console.log(`[onAuthUserCreate] Bootstrap completed for ${uid}`);
});

/**
 * ─────────────────────────────────────────────
 * EMULATOR SEED LOADER (DEV ONLY)
 * ─────────────────────────────────────────────
 */
export const seedEmulatorData = functions.https.onRequest(async (_req, res) => {
  if (!process.env.FUNCTIONS_EMULATOR) {
    res.status(403).send("Seed function is emulator-only.");
    return;
  }

  const seedDir = path.resolve(__dirname, "../../emulators/seed");
  if (!fs.existsSync(seedDir)) {
    res.status(400).send("Seed directory not found.");
    return;
  }

  const files = fs.readdirSync(seedDir).filter(f => f.endsWith(".json"));
  let writes = 0;

  for (const file of files) {
    const collectionName = file.replace(".json", "");
    const content = JSON.parse(
      fs.readFileSync(path.join(seedDir, file), "utf8")
    );

    const docs = content[collectionName];
    if (!docs) continue;

    for (const docId of Object.keys(docs)) {
      await db.collection(collectionName).doc(docId).set(docs[docId], {
        merge: false
      });
      writes++;
    }
  }

  res.send(`Seeded ${writes} documents into Firestore emulator.`);
});

/**
 * ─────────────────────────────────────────────
 * TASK COMPLETION HANDLER
 * ─────────────────────────────────────────────
 */
export const onTaskCompleted = functions.firestore
  .document("tasks/{taskId}")
  .onUpdate(async (change, context) => {
    const before = change.before.data();
    const after = change.after.data();

    if (!before || !after) return;

    // Only first transition → completed
    if (before.status === "completed" || after.status !== "completed") {
      return;
    }

    const userId = after.userId;
    if (!userId) return;

    const occurredAt =
      after.completedAt ?? new Date().toISOString();

    // 1️⃣ Emit progression event (append-only)
    const eventRef = db.collection("progressionEvents").doc();
    await eventRef.set({
      eventId: eventRef.id,
      userId,
      type: "TASK_COMPLETED",
      subtype: `${(after.estimatedDepth || "small").toUpperCase()}_TASK`,
      occurredAt,
      metadata: {
        taskId: context.params.taskId,
        projectId: after.projectId ?? null
      }
    });

    // 2️⃣ Apply authoritative growth (backend-only)
    await applyTaskCompletion({
      userId,
      depth: after.estimatedDepth || "small",
      occurredAt
    });

    console.log(
      `[onTaskCompleted] Event + growth applied for task ${context.params.taskId}`
    );
  });

/**
 * ─────────────────────────────────────────────
 * REFLECTION CREATION HANDLER
 * ─────────────────────────────────────────────
 */
export const onReflectionCreated = functions.firestore
  .document("reflections/{reflectionId}")
  .onCreate(async (snap, context) => {
    const data = snap.data();
    if (!data) return;

    const userId = data.userId;
    if (!userId) return;

    const occurredAt =
      data.createdAt ?? new Date().toISOString();

    // 1️⃣ Emit progression event
    const eventRef = db.collection("progressionEvents").doc();
    await eventRef.set({
      eventId: eventRef.id,
      userId,
      type: "REFLECTION_LOGGED",
      subtype: "USER_REFLECTION",
      occurredAt,
      metadata: {
        relatedTaskIds: data.relatedTaskIds ?? [],
        tags: data.tags ?? []
      }
    });

    // 2️⃣ Apply vitality modulation (expression only)
    await applyReflection({
      userId,
      occurredAt
    });

    console.log(
      `[onReflectionCreated] Event + vitality applied for reflection ${context.params.reflectionId}`
    );
  });
