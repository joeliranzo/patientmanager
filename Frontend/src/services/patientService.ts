import axios from "axios";

export interface Patient {
  id: number;
  firstName: string;
  lastName: string;
  socialSecurityNumber: string;
  dateOfBirth: string;
  address?: string;
  phoneNumber?: string;
  email?: string;
  createdDate: string;
  modifiedDate: string;
}

export interface CreatePatientRequest {
  firstName: string;
  lastName: string;
  socialSecurityNumber: string;
  dateOfBirth: string;
  address?: string;
  phoneNumber?: string;
  email?: string;
}

export interface UpdatePatientRequest extends Partial<CreatePatientRequest> {}

const API_BASE = "https://localhost:5001/api/patients";

export const getPatients = () => axios.get<Patient[]>(API_BASE);
export const getPatient = (id: number) => axios.get<Patient>(`${API_BASE}/${id}`);
export const createPatient = (data: CreatePatientRequest) => axios.post<number>(API_BASE, data);
export const updatePatient = (id: number, data: UpdatePatientRequest) => axios.put(`${API_BASE}/${id}`, data);
export const deletePatient = (id: number) => axios.delete(`${API_BASE}/${id}`);