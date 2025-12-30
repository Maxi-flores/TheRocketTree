import { firestore } from "./firebase";

/**
 * ─────────────────────────────────────────────
 * Progression Interpreter (Authoritative)
 * ─────────────────────────────────────────────
 * The ONLY place where growth math exists.
 */

const db = firestore;

/**
 * TreeState bounds (hard safety rails)
 */
const LIMITS = {
  vitality: { min: 0.6, max: 1.0 },
  massDelta: { min: 0.01, max: 0.08 },
  structureDelta: { min: 0.01, max: 0.1 }
};

export async function applyTaskCompletion(params: {
  userId: string;
  depth: "small" | "medium" | "deep";
  occurredAt: string;
}) {
  const { userId, depth, occurredAt } = params;

  const treeRef = db.collection("treeState").doc(userId);
  const snap = await treeRef.get();
  if (!snap.exists) return;

  const state = snap.data()!;
  let massDelta = 0.02;
  let structureDelta = 0.02;

  switch (depth) {
    case "medium":
      structureDelta = 0.04;
      break;
    case "deep":
      massDelta = 0.05;
      structureDelta = 0.08;
      break;
  }

  await treeRef.update({
    mass: state.mass + clamp(massDelta, LIMITS.massDelta),
    structure: state.structure + clamp(structureDelta, LIMITS.structureDelta),
    vitality: clamp(state.vitality + 0.01, LIMITS.vitality),
    lastUpdatedAt: occurredAt
  });
}

export async function applyReflection(params: {
  userId: string;
  occurredAt: string;
}) {
  const { userId, occurredAt } = params;

  const treeRef = db.collection("treeState").doc(userId);
  const snap = await treeRef.get();
  if (!snap.exists) return;

  const state = snap.data()!;

  await treeRef.update({
    vitality: clamp(state.vitality + 0.03, LIMITS.vitality),
    lastUpdatedAt: occurredAt
  });
}

function clamp(value: number, range: { min: number; max: number }) {
  return Math.min(range.max, Math.max(range.min, value));
}
