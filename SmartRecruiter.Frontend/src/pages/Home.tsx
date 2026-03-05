import { Link } from 'react-router-dom';

export function Home() {
    return (
        <div className="flex h-screen flex-col items-center justify-center bg-gray-50 text-gray-900">
            <h1 className="text-5xl font-bold mb-6">SmartRecruiter AI</h1>
            <p className="mb-8 text-xl text-gray-600">The intelligent way to hire.</p>

            <div className="space-x-4">
                <Link to="/login" className="px-6 py-3 bg-blue-600 text-white rounded-lg shadow hover:bg-blue-700 transition">Log In</Link>
                <Link to="/register" className="px-6 py-3 bg-gray-200 text-gray-800 rounded-lg shadow hover:bg-gray-300 transition">Register</Link>
            </div>
        </div>
    );
}