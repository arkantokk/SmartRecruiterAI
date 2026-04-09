import { create } from "zustand"
import { authService } from "../features/auth/authService.ts";

interface IAuthStore {
    isAuthenticated: boolean;
    isLoading: boolean;
    checkAuth: () => Promise<void>;
    loginSuccess: () => void;
    logoutSuccess: () => void;
}


export const useAuthStore = create<IAuthStore>((set) => ({
    isAuthenticated: false,
    isLoading: true,
    checkAuth: async () => {
        try {
            await authService.me();
            set({ isAuthenticated: true });
        } catch (e) {
            set({ isAuthenticated: false });
        } finally {
            set({ isLoading: false });
        }
    },

    loginSuccess: () => {
        set({ isAuthenticated: true });
    },

    logoutSuccess: () => {
        set({ isAuthenticated: false });
    }
}));