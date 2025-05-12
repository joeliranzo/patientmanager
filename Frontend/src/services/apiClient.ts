import axios from "axios";
import config from "../config";

const apiClient = axios.create({
	baseURL: `${config.apiBaseUrl}/api`,
});

apiClient.interceptors.response.use(
	response => response,
	async error => {
		const originalRequest = error.config;
		if (error.response?.status === 401 && !originalRequest._retry) {
			originalRequest._retry = true;
			const refreshToken = sessionStorage.getItem("refreshToken");
			if (refreshToken) {
				try {
					const res = await axios.post(
						`${config.apiBaseUrl}/api/auth/refresh`,
						{ refreshToken }
					);
					const newToken = res.data.token;
					sessionStorage.setItem("token", newToken);
					apiClient.defaults.headers.common["Authorization"] = `Bearer ${newToken}`;
					originalRequest.headers["Authorization"] = `Bearer ${newToken}`;
					return apiClient(originalRequest);
				} catch {
					sessionStorage.removeItem("token");
					sessionStorage.removeItem("refreshToken");
					window.location.href = "/";
				}
			}
		}
		return Promise.reject(error);
	}
);

export default apiClient;
