import React, { useState, useEffect } from "react";
import PatientList from "./pages/PatientList";
import PatientForm from "./pages/PatientForm";

export default function ProtectedApp() {
  const [editId, setEditId] = useState<number | null>(null);
  const [refresh, setRefresh] = useState(false);

  return (
    <div>
      <h1>Patient Manager</h1>
      <PatientForm patientId={editId} onSaved={() => { setEditId(null); setRefresh(!refresh); }} />
      <PatientList onEdit={(id: number) => setEditId(id)} key={refresh ? "refresh-true" : "refresh-false"} />
    </div>
  );
}