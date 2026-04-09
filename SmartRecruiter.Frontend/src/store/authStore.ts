import { create } from "zustand"
import { authService } from "../features/auth/authService.ts";

interface IAuthStore {
    isAuthenticated: boolean;
    isLoading: boolean;
    checkAuth: () => Promise<void>;
    loginSuccess: () => Promise<void>;
    logoutSuccess: () => void;
    email: string;
}


export const useAuthStore = create<IAuthStore>((set, get) => ({
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

    loginSuccess: async () => {
        set({ isAuthenticated: true });
        await get().checkAuth();
    },

    logoutSuccess: () => {
        set({ isAuthenticated: false, email: "" });
    }
}));