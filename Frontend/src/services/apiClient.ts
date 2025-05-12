import axios from "axios";
import config from "../config";

const apiClient = axios.create({
  baseURL: `${config.apiBaseUrl}/api`
});

function logoutAndRedirect() {
  sessionStorage.clear();
  window.location.replace("/");
}

apiClient.interceptors.response.use(
  response => response,
  async error => {
    const original = error.config;
    if (error.response?.status === 401 && !original._retry) {
      original._retry = true;
      const refreshToken = sessionStorage.getItem("refreshToken");
      if (refreshToken) {
        try {
          const res = await apiClient.post("/auth/refresh", { refresh_token: refreshToken });
          const { token, refresh_token: newRefresh } = res.data;
          sessionStorage.setItem("token", token);
          sessionStorage.setItem("refreshToken", newRefresh);
          apiClient.defaults.headers.common["Authorization"] = `Bearer ${token}`;
          original.headers["Authorization"] = `Bearer ${token}`;
          return apiClient(original);
        } catch {
          logoutAndRedirect();
        }
      } else {
        logoutAndRedirect();
      }
    } else if (error.response?.status === 401) {
      logoutAndRedirect();
    }
    return Promise.reject(error);
  }
);

export default apiClient;
