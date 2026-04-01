import React from 'react';

interface ScoreCircleProps {
    score: number;
}

export const ScoreCircle: React.FC<ScoreCircleProps> = ({ score }) => {
    const radius = 28;
    const circumference = 2 * Math.PI * radius;
    const strokeDashoffset = circumference - (score / 100) * circumference;

    const colorClass =
        score >= 80 ? 'text-emerald-500' :
            score >= 50 ? 'text-amber-500' : 'text-rose-500';

    return (
        <div className="relative flex items-center justify-center w-20 h-20">
            <svg className="w-full h-full transform -rotate-90">
                <circle
                    cx="40" cy="40" r={radius}
                    stroke="currentColor" strokeWidth="6" fill="transparent"
                    className="text-gray-100"
                />
                <circle
                    cx="40" cy="40" r={radius}
                    stroke="currentColor" strokeWidth="6" fill="transparent"
                    strokeDasharray={circumference}
                    strokeDashoffset={strokeDashoffset}
                    className={`${colorClass} transition-all duration-1000 ease-out`}
                    strokeLinecap="round"
                />
            </svg>
            <div className="absolute flex flex-col items-center justify-center">
                <span className="text-xl font-bold text-gray-800">{score}</span>
            </div>
        </div>
    );
};