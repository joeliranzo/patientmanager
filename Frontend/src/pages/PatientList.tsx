import React, { useEffect, useState } from "react";
import { Button, List, Typography, Popconfirm, message, Card } from "antd";
import { getPatients, deletePatient } from "../services/patientService";

interface Props {
  onEdit: (id: number) => void;
}

export default function PatientList({ onEdit }: Props) {
  const [patients, setPatients] = useState<any[]>([]);

  const load = async () => {
    const res = await getPatients();
    setPatients(res.data);
  };

  const handleDelete = async (id: number) => {
    await deletePatient(id);
    message.success("Deleted");
    load();
  };

  useEffect(() => {
    load();
  }, []);

  return (
    <Card title="Patient List">
      <List
        bordered
        dataSource={patients}
        renderItem={(p) => (
          <List.Item
            actions={[
              <Button key="edit" onClick={() => onEdit(p.id)}>Edit</Button>,
              <Popconfirm key="delete" title="Are you sure?" onConfirm={() => handleDelete(p.id)}>
                <Button danger>Delete</Button>
              </Popconfirm>
            ]}
          >
            <Typography.Text>{p.firstName} {p.lastName}</Typography.Text> - {p.socialSecurityNumber}
          </List.Item>
        )}
      />
    </Card>
  );
}