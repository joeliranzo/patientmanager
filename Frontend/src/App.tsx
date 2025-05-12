import React, { useState, useEffect } from "react";
import LoginPage from "./pages/LoginPage";
import ProtectedApp from "./ProtectedApp";
import axios from "axios";

function App() {
  const [authenticated, setAuthenticated] = useState(false);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      setAuthenticated(true);
    }
  }, []);

  const handleLogin = () => {
    const token = localStorage.getItem("token");
    axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    setAuthenticated(true);
  };

  return authenticated ? <ProtectedApp /> : <LoginPage onLogin={handleLogin} />;
}

export default App;