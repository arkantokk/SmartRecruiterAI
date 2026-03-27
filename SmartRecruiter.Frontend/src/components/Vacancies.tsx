import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {vacanciesService} from "../features/vacancies/vacanciesService.ts";
import type {JobVacancy} from "../models/JobVacancy/JobVacancy.ts";


export const Vacancies = () => {
    const queryClient = useQueryClient();
    const [title, setTitle] = useState("");
    const [aiPromptTemplate, setAiPromptTemplate] = useState("");

    const { data, isLoading, isError } = useQuery({
        queryKey: ['vacancies'],
        queryFn: vacanciesService.getVacancies
    });

    const { mutate, isPending } = useMutation({
        mutationFn: vacanciesService.postVacancy,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['vacancies'] });
            setTitle("");
            setAiPromptTemplate("");
        }
    });

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        if (!title.trim() || !aiPromptTemplate.trim()) return;

        const newVacancy: JobVacancy = { title, aiPromptTemplate };
        mutate(newVacancy);
    };

    return (
        <div className="p-8 max-w-4xl mx-auto">
            <h1 className="text-2xl font-bold mb-6">Job Vacancies</h1>

            <form onSubmit={handleSubmit} className="bg-white p-6 rounded-lg shadow-md mb-8">
                <h2 className="text-xl font-semibold mb-4">Create New Vacancy</h2>
                <div className="mb-4">
                    <label className="block text-gray-700 font-medium mb-2">Title</label>
                    <input
                        type="text"
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                        className="w-full border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                        required
                    />
                </div>
                <div className="mb-4">
                    <label className="block text-gray-700 font-medium mb-2">AI Prompt Template</label>
                    <textarea
                        value={aiPromptTemplate}
                        onChange={(e) => setAiPromptTemplate(e.target.value)}
                        className="w-full border border-gray-300 rounded px-3 py-2 h-32 focus:outline-none focus:ring-2 focus:ring-blue-500"
                        required
                    />
                </div>
                <button
                    type="submit"
                    disabled={isPending}
                    className="bg-blue-600 text-white px-4 py-2 rounded font-medium hover:bg-blue-700 disabled:bg-blue-300 transition-colors"
                >
                    {isPending ? "Creating..." : "Create Vacancy"}
                </button>
            </form>

            <div>
                <h2 className="text-xl font-semibold mb-4">Active Vacancies</h2>
                {isLoading ? (
                    <div className="text-gray-500">Loading vacancies...</div>
                ) : isError ? (
                    <div className="text-red-500">Failed to load vacancies.</div>
                ) : data && data.length > 0 ? (
                    <div className="grid gap-4">
                        {data.map((vacancy) => (
                            <div key={vacancy.id} className="bg-white p-5 rounded-lg shadow border border-gray-200">
                                <h3 className="text-lg font-bold text-gray-900">{vacancy.title}</h3>
                                <p className="text-gray-600 mt-2 text-sm whitespace-pre-wrap bg-gray-50 p-3 rounded border border-gray-100">
                                    {vacancy.aiPromptTemplate}
                                </p>
                            </div>
                        ))}
                    </div>
                ) : (
                    <div className="text-gray-500 bg-white p-6 rounded-lg border border-dashed border-gray-300 text-center">
                        No vacancies found. Create one above.
                    </div>
                )}
            </div>
        </div>
    );
};