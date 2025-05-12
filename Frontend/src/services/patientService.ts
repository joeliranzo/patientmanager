import config from "../config";
import apiClient from "./apiClient";

export interface Patient {
  id: number;
  first_name: string;
  last_name: string;
  social_security_number: string;
  date_of_birth: string;
  address?: string;
  phone_number?: string;
  email?: string;
  created_date: string;
  modified_date: string;
}

export interface CreatePatientRequest {
  first_name: string;
  last_name: string;
  social_security_number: string;
  date_of_birth: string;
  address?: string;
  phone_number?: string;
  email?: string;
}

export interface PagedResult<T> {
  items: T[];
  total_count: number;
  page: number;
  page_size: number;
  total_pages: number;
}


export interface PatientQueryParams {
  first_name?: string;
  last_name?: string;
  email?: string;
  page?: number;
  page_size?: number;
  sort_by?: string;
  sort_order?: "asc" | "desc";
}

export interface UpdatePatientRequest extends Partial<CreatePatientRequest> {}

const API_BASE = `${config.apiBaseUrl}/api/patients`;

export const getPatients = () => apiClient.get<Patient[]>(API_BASE);
export const getPatient = (id: number) => apiClient.get<Patient>(`${API_BASE}/${id}`);
export const createPatient = (data: CreatePatientRequest) => apiClient.post<number>(API_BASE, data);
export const updatePatient = (id: number, data: UpdatePatientRequest) => apiClient.put(`${API_BASE}/${id}`, data);
export const deletePatient = (id: number) => apiClient.delete(`${API_BASE}/${id}`);
export const queryPatients = (params: PatientQueryParams) => 
  apiClient.get<PagedResult<Patient>>(`${API_BASE}/search`, { params });
