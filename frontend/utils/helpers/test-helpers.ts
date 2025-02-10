import { vi } from 'vitest';

/**
 * Creates a mock function that resolves after a specified delay.
 *
 * This function is useful for simulating asynchronous behavior in tests, such as API calls or other delayed operations.
 * The mock function resolves with `undefined` after the specified delay.
 *
 * @param delay - The delay in seconds before the mock function resolves.
 * @returns A mock function that resolves with `undefined` after the specified delay.
 *
 * @example
 *
 * it('should handle delayed action', async () => {
 *   const handleClick = createDelayedMock(1);
 *   render(<Button action={handleClick}>Click Me</Button>);
 *
 *   await userEvent.click(screen.getByRole('button', { name: 'Click Me' }));
 *   await waitFor(() => expect(handleClick).toHaveBeenCalledOnce());
 * });
 */
export const createDelayedMock = (delay: number) =>
    vi
        .fn()
        .mockImplementation(
            () =>
                new Promise<void>((res) =>
                    setTimeout(() => res(undefined), delay * 1000)
                )
        );
