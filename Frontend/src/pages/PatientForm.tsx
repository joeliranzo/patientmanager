import React, { useEffect, useState } from "react";
import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  Input,
  Row,
  Space,
  message
} from "antd";
import { createPatient, getPatient, updatePatient } from "../services/patientService";
import dayjs from "dayjs";
import { faker } from "@faker-js/faker";

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
          firstName: res.data.first_name,
          lastName: res.data.last_name,
          socialSecurityNumber: res.data.social_security_number,
          dateOfBirth: dayjs(res.data.date_of_birth),
          address: res.data.address,
          phoneNumber: res.data.phone_number,
          email: res.data.email
        });
      });
    } else {
      form.resetFields();
    }
  }, [patientId]);

  const onFinish = async (values: any) => {
    setLoading(true);
    const payload = {
      first_name: values.firstName,
      last_name: values.lastName,
      social_security_number: values.socialSecurityNumber,
      date_of_birth: values.dateOfBirth.format("YYYY-MM-DD"),
      address: values.address,
      phone_number: values.phoneNumber,
      email: values.email
    };
    try {
      if (patientId) {
        await updatePatient(patientId, payload);
        message.success("Patient updated");
      } else {
        await createPatient(payload);
        message.success("Patient created");
      }
      form.resetFields();
      onSaved();
    } catch {
      message.error("Something went wrong");
    } finally {
      setLoading(false);
    }
  };

  const handleMockFill = () => {
    form.setFieldsValue({
      firstName: faker.person.firstName(),
      lastName: faker.person.lastName(),
      socialSecurityNumber: faker.helpers.fromRegExp("[0-9]{3}-[0-9]{2}-[0-9]{4}"),
      dateOfBirth: dayjs(faker.date.birthdate({ min: 18, max: 65, mode: "age" })),
      email: faker.internet.email(),
      phoneNumber: faker.phone.number(),
      address: faker.location.streetAddress()
    });
  };

  return (
    <Card title={patientId ? "Edit Patient" : "New Patient"} style={{ marginBottom: 24 }}>
      <Form
        form={form}
        layout="vertical"
        onFinish={onFinish}
      >
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="firstName"
              label="First Name"
              rules={[{ required: true, message: "Please enter first name" }]}
            >
              <Input />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item
              name="lastName"
              label="Last Name"
              rules={[{ required: true, message: "Please enter last name" }]}
            >
              <Input />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="socialSecurityNumber"
              label="SSN"
              rules={[{ required: true, message: "SSN is required" }]}
            >
              <Input placeholder="123-45-6789" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item
              name="dateOfBirth"
              label="Date of Birth"
              rules={[{ required: true, message: "Select date of birth" }]}
            >
              <DatePicker style={{ width: "100%" }} />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="email" label="Email" rules={[{ type: "email", message: "Invalid email" }]}>
              <Input />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="phoneNumber" label="Phone">
              <Input />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item name="address" label="Address">
          <Input.TextArea rows={2} />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit" loading={loading}>
              {patientId ? "Update" : "Create"}
            </Button>
            <Button onClick={() => form.resetFields()} disabled={loading}>
              Clear
            </Button>
            <Button onClick={handleMockFill} disabled={loading}>
              Fill Mock
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </Card>
  );
}