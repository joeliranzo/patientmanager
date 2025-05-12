import React, { useEffect, useState } from "react";
import { Button, Form, Input, DatePicker, Card } from "antd";
import { createPatient, getPatient, updatePatient } from "../services/patientService";
import dayjs from "dayjs";

interface Props {
  patientId: number | null;
  onSaved: () => void;
}

export default function PatientForm({ patientId, onSaved }: Props) {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (patientId) {
      getPatient(patientId).then((res) => {
        form.setFieldsValue({
          ...res.data,
          dateOfBirth: dayjs(res.data.dateOfBirth)
        });
      });
    } else {
      form.resetFields();
    }
  }, [patientId]);

  const onFinish = async (values: any) => {
    setLoading(true);
    const payload = { ...values, dateOfBirth: values.dateOfBirth.format("YYYY-MM-DD") };
    patientId ? await updatePatient(patientId, payload) : await createPatient(payload);
    setLoading(false);
    form.resetFields();
    onSaved();
  };

  return (
    <Card title={patientId ? "Edit Patient" : "New Patient"} style={{ marginBottom: 24 }}>
      <Form form={form} layout="vertical" onFinish={onFinish}>
        <Form.Item name="firstName" label="First Name" rules={[{ required: true }]}>
          <Input />
        </Form.Item>
        <Form.Item name="lastName" label="Last Name" rules={[{ required: true }]}>
          <Input />
        </Form.Item>
        <Form.Item name="socialSecurityNumber" label="SSN" rules={[{ required: true }]}>
          <Input />
        </Form.Item>
        <Form.Item name="dateOfBirth" label="Date of Birth" rules={[{ required: true }]}>
          <DatePicker style={{ width: "100%" }} />
        </Form.Item>
        <Form.Item name="address" label="Address">
          <Input />
        </Form.Item>
        <Form.Item name="phoneNumber" label="Phone">
          <Input />
        </Form.Item>
        <Form.Item name="email" label="Email">
          <Input type="email" />
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" loading={loading}>
            {patientId ? "Update" : "Create"}
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
}