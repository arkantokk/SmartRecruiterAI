import { authSchema } from "../authSchema";
import type { authFormValues } from "../authSchema";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { authService } from "../authService";
import { useNavigate, Link } from "react-router-dom";
import { useState } from "react";
import {useAuthStore} from "../../../store/authStore.ts";
import {type CredentialResponse, GoogleLogin, GoogleOAuthProvider} from "@react-oauth/google";

export const RegisterForm = () => {
    const navigate = useNavigate();
    const [serverError, setServerError] = useState<string | null>(null);
    const loginSuccess = useAuthStore((state) => state.loginSuccess)
    const { register, handleSubmit, formState: { errors } } = useForm<authFormValues>({
        resolver: zodResolver(authSchema),
    });

    const onSubmit = async (values: authFormValues) => {
        try {
            setServerError(null);
            await authService.register(values);
            navigate("/login");
        } catch (error: any) {
            if (error.response?.data) {
                const errorData = error.response.data;
                const errorMessage = typeof errorData === 'string'
                    ? errorData
                    : JSON.stringify(errorData);
                setServerError(errorMessage);
            } else {
                setServerError("An unexpected error occurred. Please try again.");
            }
        }
    }

    const googleLogin = async (credentialResponse: CredentialResponse) => {
        try{
            const result = await authService.googleLogin(credentialResponse);
            if (result.token) {
                localStorage.setItem("token", result.token);
                loginSuccess();
                navigate("/candidates");
            }
        } catch (e) {
            console.error(e);
        }
    }

    return (
        <div className="flex items-center justify-center min-h-screen bg-gray-50 px-4">
            <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-8">
                <h2 className="text-3xl font-extrabold text-gray-900 text-center mb-8">
                    Create Account
                </h2>

                {serverError && (
                    <div className="mb-6 p-4 bg-red-50 border border-red-200 text-red-600 rounded-lg text-sm break-words">
                        {serverError}
                    </div>
                )}

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div className="flex flex-col gap-1.5">
                        <label className="text-sm font-semibold text-gray-700">
                            Email Address
                        </label>
                        <input
                            type="email"
                            placeholder="you@example.com"
                            className={`w-full px-4 py-2.5 rounded-lg border bg-gray-50 focus:bg-white outline-none transition-all duration-200 ${
                                errors.email
                                    ? "border-red-500 focus:ring-2 focus:ring-red-200"
                                    : "border-gray-300 focus:border-blue-600 focus:ring-2 focus:ring-blue-100"
                            }`}
                            {...register("email")}
                        />
                        {errors.email && (
                            <span className="text-sm text-red-600 font-medium">
                                {errors.email.message}
                            </span>
                        )}
                    </div>

                    <div className="flex flex-col gap-1.5">
                        <label className="text-sm font-semibold text-gray-700">
                            Password
                        </label>
                        <input
                            type="password"
                            placeholder="••••••••"
                            className={`w-full px-4 py-2.5 rounded-lg border bg-gray-50 focus:bg-white outline-none transition-all duration-200 ${
                                errors.password
                                    ? "border-red-500 focus:ring-2 focus:ring-red-200"
                                    : "border-gray-300 focus:border-blue-600 focus:ring-2 focus:ring-blue-100"
                            }`}
                            {...register("password")}
                        />
                        {errors.password && (
                            <span className="text-sm text-red-600 font-medium">
                                {errors.password.message}
                            </span>
                        )}
                    </div>

                    <button
                        type="submit"
                        className="w-full mt-2 bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-4 rounded-lg shadow-md hover:shadow-lg transition-all duration-200 active:transform active:scale-95 focus:outline-none focus:ring-4 focus:ring-blue-200"
                    >
                        Register
                    </button>

                    <div className="text-center mt-4">
                        <span className="text-sm text-gray-600">Already have an account? </span>
                        <Link to="/login" className="text-sm font-bold text-blue-600 hover:underline">
                            Login here
                        </Link>
                        <GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_CLIENT_ID}>

                            <GoogleLogin
                                onSuccess={credentialResponse => googleLogin(credentialResponse)}
                                onError={() => {
                                    console.log('something went wrong');
                                }}
                            />

                        </GoogleOAuthProvider>
                    </div>

                </form>
            </div>
        </div>
    )
}

export default RegisterForm;