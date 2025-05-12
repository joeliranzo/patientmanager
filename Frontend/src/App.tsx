import { useState, useEffect } from "react";
import LoginPage from "./pages/LoginPage";
import ProtectedApp from "./ProtectedApp";
import apiClient from "./services/apiClient";

function App() {
  const [authenticated, setAuthenticated] = useState(false);

  useEffect(() => {
    const token = sessionStorage.getItem("token");
    if (token) {
      apiClient.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      setAuthenticated(true);
    }
  }, []);

  const handleLogin = () => {
    const token = sessionStorage.getItem("token");
    apiClient.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    setAuthenticated(true);
  };

  return authenticated ? <ProtectedApp /> : <LoginPage onLogin={handleLogin} />;
}

export default App;
