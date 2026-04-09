import { useNavigate } from "react-router-dom";
import {authService} from "../features/auth/authService.ts";
import {useAuthStore} from "../store/authStore.ts";
import {useQueryClient} from "@tanstack/react-query";
export const Header = () => {
    const navigate = useNavigate();
    const query = useQueryClient();
    const userEmail = useAuthStore((state) => state.email);
    const initial = userEmail ? userEmail.charAt(0).toUpperCase() : "U";
    const logoutSuccess = useAuthStore((state) => state.logoutSuccess);
    const handleLogout = async () => {
        await authService.logout();
        query.clear();
        logoutSuccess();
        localStorage.removeItem("token");
        navigate("/login");
    };

    return (
        <header className="sticky top-0 z-50 w-full bg-white/80 backdrop-blur-md border-b border-gray-200 shadow-sm">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="flex justify-between items-center h-16">
                    <div className="flex-shrink-0 flex items-center gap-3">
                        <div className="w-9 h-9 bg-gradient-to-br from-blue-600 to-indigo-600 rounded-xl flex items-center justify-center shadow-md shadow-blue-200">
                            <span className="text-white font-extrabold text-xl leading-none">S</span>
                        </div>
                        <span className="text-2xl font-extrabold bg-clip-text text-transparent bg-gradient-to-r from-blue-600 to-indigo-600 tracking-tight cursor-default">
                            SmartRecruiter
                        </span>
                    </div>

                    <div className="flex items-center gap-4">

                        <div className="hidden sm:flex items-center gap-3 px-3 py-1.5 rounded-full border border-gray-100 bg-gray-50 text-gray-700 hover:bg-gray-100 transition-colors cursor-default">
                            <div className="w-7 h-7 rounded-full bg-gradient-to-tr from-blue-500 to-indigo-400 flex items-center justify-center text-white font-bold text-xs shadow-inner">
                                {initial}
                            </div>
                            <span className="text-sm font-medium tracking-wide pr-1">
                                {userEmail}
                            </span>
                        </div>

                        <div className="hidden sm:block h-6 w-px bg-gray-200"></div>

                        <button
                            onClick={handleLogout}
                            className="group flex items-center gap-2 px-4 py-2 text-sm font-bold text-gray-600 hover:text-red-600 hover:bg-red-50 rounded-xl transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-red-100 active:scale-95"
                        >
                            <svg
                                className="w-5 h-5 transition-transform duration-200 group-hover:-translate-x-0.5"
                                fill="none"
                                stroke="currentColor"
                                viewBox="0 0 24 24"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth={2.5}
                                    d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"
                                />
                            </svg>
                            <span>Logout</span>
                        </button>

                    </div>
                </div>
            </div>
        </header>
    );
};

export default Header;