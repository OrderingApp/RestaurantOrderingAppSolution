import { render, fireEvent, screen } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import { Button } from './Index'; // Adjust the import based on the Button component's export type

describe('Button Component', () => {
    it('renders the button correctly', () => {
        render(<Button>Click Me 1</Button>);
        const button = screen.getByText('Click Me 1');
        expect(button).not.toBeNull();
    });

    it('handles click events', () => {
        const handleClick = vi.fn();
        render(<Button onClick={handleClick}>Click Me 2</Button>);
        const button = screen.getByText('Click Me 2');
        fireEvent.click(button);
        expect(handleClick).toHaveBeenCalledOnce();
    });
});
