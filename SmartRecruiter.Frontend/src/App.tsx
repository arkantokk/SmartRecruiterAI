import React from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import LoginForm from "./features/auth/components/LoginForm";
import RegisterForm from "./features/auth/components/RegisterForm";
import Candidates from "./pages/Candidates";

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
    const token = localStorage.getItem("token");
    if (!token) {
        return <Navigate to="/login" replace />;
    }
    return <>{children}</>;
};

const PublicRoute = ({ children }: { children: React.ReactNode }) => {
    const token = localStorage.getItem("token");
    if (token) {
        return <Navigate to="/candidates" replace />;
    }
    return <>{children}</>;
};

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route
                    path="/login"
                    element={
                        <PublicRoute>
                            <LoginForm />
                        </PublicRoute>
                    }
                />

                <Route
                    path="/register"
                    element={
                        <PublicRoute>
                            <RegisterForm />
                        </PublicRoute>
                    }
                />

                <Route
                    path="/candidates"
                    element={
                        <ProtectedRoute>
                            <Candidates />
                        </ProtectedRoute>
                    }
                />

                <Route path="*" element={<Navigate to="/candidates" replace />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;