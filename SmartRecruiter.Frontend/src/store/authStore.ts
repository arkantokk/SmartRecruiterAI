import { create } from "zustand"
import { authService } from "../features/auth/authService.ts";

interface IAuthStore {
    isAuthenticated: boolean;
    isLoading: boolean;
    checkAuth: () => Promise<void>;
    loginSuccess: () => void;
    logoutSuccess: () => void;
    email: string;
}


export const useAuthStore = create<IAuthStore>((set) => ({
    isAuthenticated: false,
    isLoading: true,
    email: "",
    checkAuth: async () => {
        try {
            const result = await authService.me();
            set({ isAuthenticated: true , email: result.user});
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