import * as admin from "firebase-admin";

/**
 * ─────────────────────────────────────────────
 * Firebase Admin Singleton
 * ─────────────────────────────────────────────
 * This is the ONLY place firebase-admin is initialized.
 * All other modules must import Firestore from here.
 */

if (admin.apps.length === 0) {
  admin.initializeApp();
}

export const firestore = admin.firestore();
