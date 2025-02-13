import { describe, it, expect, vi } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import { userEvent } from '@testing-library/user-event';

import { Button } from './Button';
import { createDelayedMock } from '@/utils/helpers/test-helpers';

describe('Button Component', () => {
    it('handles regular onClick callback', async () => {
        const handleClick = vi.fn();
        render(<Button onClick={handleClick}>Click Me</Button>);

        await userEvent.click(screen.getByRole('button', { name: 'Click Me' }));

        expect(handleClick).toHaveBeenCalledOnce();
    });

    it('handles server action callback', async () => {
        const handleClick = vi.fn().mockResolvedValue(undefined);
        render(<Button action={handleClick}>Click Me</Button>);

        await userEvent.click(screen.getByRole('button', { name: 'Click Me' }));

        expect(handleClick).toHaveBeenCalledOnce();

        await expect(handleClick()).resolves.not.toThrow();
    });

    it('is disabled only when action is in loading state', async () => {
        const handleClick = createDelayedMock(1);
        render(<Button action={handleClick}>Click Me</Button>);
        const button = screen.getByRole('button', { name: 'Click Me' });

        await userEvent.click(button);

        expect(button).toBeDisabled();

        await waitFor(() => expect(handleClick).toHaveBeenCalledOnce());
        await waitFor(() => expect(button).toBeEnabled());
    });
});
