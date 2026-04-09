import { useEffect } from "react";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";

import LoginForm from "./features/auth/components/LoginForm";
import RegisterForm from "./features/auth/components/RegisterForm";
import Candidates from "./pages/Candidates";
import { VacancyDetails } from "./pages/VacancyDetails";
import {useAuthStore} from "./store/authStore.ts";

function App() {
    const {isAuthenticated, isLoading, checkAuth} = useAuthStore();
    useEffect(() => {
        if(!isAuthenticated){
            checkAuth();
        }
    }, []);
    if (isLoading) {
        return (
            <div className="flex items-center justify-center h-screen bg-gray-50">
                <div className="w-16 h-16 border-4 border-dashed rounded-full animate-spin border-blue-500"></div>
            </div>
        );
    }

    return (
        <BrowserRouter>
            <Routes>
                <Route
                    path="/login"
                    element={isAuthenticated ? <Navigate to="/candidates" replace /> : <LoginForm />}
                />
                <Route
                    path="/register"
                    element={isAuthenticated ? <Navigate to="/candidates" replace /> : <RegisterForm />}
                />

                <Route
                    path="/candidates"
                    element={isAuthenticated ? <Candidates /> : <Navigate to="/login" replace />}
                />
                <Route
                    path="/vacancies/:id"
                    element={isAuthenticated ? <VacancyDetails /> : <Navigate to="/login" replace />}
                />
                <Route path="*" element={<Navigate to={isAuthenticated ? "/candidates" : "/login"} replace />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;