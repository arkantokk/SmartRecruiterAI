import {render, screen} from '@testing-library/react';
import {ScoreCircle} from './ScoreCircle.tsx';
import {describe, it, expect} from "vitest";

describe('ScoreCircle.ts', () => {
    it('renders correctly', () => {
        render(<ScoreCircle score={55}/>);
        const headingElement = screen.getByText("55");
        expect(headingElement).toBeInTheDocument();
    });


    it('has correct classes', () => {
        const {container} = render(<ScoreCircle score={55}/>);
        const coloredCircle = container.querySelectorAll('circle')[1];
        expect(coloredCircle).toHaveClass('text-amber-500');
    });
});