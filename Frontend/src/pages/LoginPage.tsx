import React, { useState } from "react";
import { Button, Form, Input, Typography, message, Card } from "antd";
import axios from "axios";

interface LoginProps {
  onLogin: () => void;
}

export default function LoginPage({ onLogin }: LoginProps) {
  const [loading, setLoading] = useState(false);

  const handleLogin = async (values: { email: string; password: string }) => {
    setLoading(true);
    try {
      const res = await axios.post("https://localhost:5001/api/auth/login", values);
      localStorage.setItem("token", res.data.token);
      axios.defaults.headers.common["Authorization"] = `Bearer ${res.data.token}`;
      onLogin();
    } catch {
      message.error("Invalid email or password");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ display: "flex", height: "100vh", justifyContent: "center", alignItems: "center" }}>
      <Card title="Login" style={{ width: 350 }}>
        <Form layout="vertical" onFinish={handleLogin}>
          <Form.Item label="Email" name="email" rules={[{ required: true, type: "email" }]}>
            <Input />
          </Form.Item>
          <Form.Item label="Password" name="password" rules={[{ required: true }]}>
            <Input.Password />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" block loading={loading}>
              Login
            </Button>
          </Form.Item>
        </Form>
        <Typography.Text type="secondary">admin@demo.com / 123456</Typography.Text>
      </Card>
    </div>
  );
}