import { useNavigate } from "react-router-dom";
import {authService} from "../features/auth/authService.ts";
export const Header = () => {
    const navigate = useNavigate();
    const handleLogout = async () => {
        await authService.logout();
        localStorage.removeItem("token");
        navigate("/login");
    };

    return (
        <header className="bg-white shadow-sm border-b px-8 py-4 flex justify-between items-center">
            <h1 className="text-2xl font-extrabold text-blue-600 tracking-tight">
                SmartRecruiter
            </h1>
            <button
                onClick={handleLogout}
                className="px-4 py-2 text-sm font-semibold text-gray-600 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
            >
                Logout
            </button>
        </header>
    );
};

export default Header;