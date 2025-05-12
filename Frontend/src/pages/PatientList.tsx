import React, { useEffect, useState } from "react";
import {
  Button,
  Card,
  Input,
  Popconfirm,
  Table,
  Typography,
  message,
  Space
} from "antd";
import type { ColumnsType, TablePaginationConfig } from "antd/es/table";
import { queryPatients, deletePatient } from "../services/patientService";

interface Props {
  onEdit: (id: number) => void;
}

export default function PatientList({ onEdit }: Props) {
  const [data, setData] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [filters, setFilters] = useState({
    firstName: "",
    lastName: "",
    email: ""
  });
  const [pagination, setPagination] = useState<TablePaginationConfig>({
    current: 1,
    pageSize: 5
  });
  const [sorter, setSorter] = useState<{ field?: string; order?: string }>({});

  const loadData = async () => {
    setLoading(true);
    try {
      const res = await queryPatients({
        page: pagination.current,
        page_size: pagination.pageSize,
        first_name: filters.firstName,
        last_name: filters.lastName,
        email: filters.email,
        sort_by: sorter.field,
        sort_order: sorter.order === "ascend" ? "asc" : "desc"
      });
      setData(res.data.items);
      setTotal(res.data.total_count);
    } catch {
      message.error("Failed to load patients");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [pagination.current, pagination.pageSize, sorter]);

  const onDelete = async (id: number) => {
    await deletePatient(id);
    message.success("Deleted");
    loadData();
  };

  const onTableChange = (pagination: TablePaginationConfig, _: any, sorter: any) => {
    setPagination(pagination);
    setSorter(sorter);
  };

  const columns: ColumnsType<any> = [
    {
      title: "Full Name",
      dataIndex: "first_name",
      key: "first_name",
      sorter: true,
      render: (_, record) => (
        <Typography.Text strong>
          {record.first_name} {record.last_name}
        </Typography.Text>
      )
    },
    {
      title: "SSN",
      dataIndex: "social_security_number",
      key: "ssn"
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
      sorter: true
    },
    {
      title: "Phone",
      dataIndex: "phone_number",
      key: "phone"
    },
    {
      title: "Actions",
      key: "actions",
      render: (_, record) => (
        <Space>
          <Button onClick={() => onEdit(record.id)}>Edit</Button>
          <Popconfirm title="Delete?" onConfirm={() => onDelete(record.id)}>
            <Button danger>Delete</Button>
          </Popconfirm>
        </Space>
      )
    }
  ];

  return (
    <Card
      title="Patient List"
      extra={
        <Space>
          <Input
            allowClear
            placeholder="First name"
            onChange={(e) => setFilters({ ...filters, firstName: e.target.value })}
            style={{ width: 150 }}
          />
          <Input
            allowClear
            placeholder="Last name"
            onChange={(e) => setFilters({ ...filters, lastName: e.target.value })}
            style={{ width: 150 }}
          />
          <Input
            allowClear
            placeholder="Email"
            onChange={(e) => setFilters({ ...filters, email: e.target.value })}
            style={{ width: 200 }}
          />
          <Button onClick={() => { setPagination({ ...pagination, current: 1 }); loadData(); }}>
            Search
          </Button>
        </Space>
      }
    >
      <Table
        rowKey="id"
        columns={columns}
        dataSource={data}
        loading={loading}
        pagination={{ ...pagination, total }}
        onChange={onTableChange}
      />
    </Card>
  );
}