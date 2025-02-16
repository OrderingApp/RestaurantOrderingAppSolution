import type { Meta, StoryObj } from '@storybook/react';
import { userEvent, within, expect } from '@storybook/test';
import DateCalendar from './DateCalendar';

const meta: Meta<typeof DateCalendar> = {
    title: 'Components/shared/DateCalendar',
    component: DateCalendar,
    tags: ['autodocs'],
    parameters: {
        docs: {
            description: {
                component: 'A customizable DateCalnedar component ',
            },
        },
    },
    argTypes: {
        variant: {
            control: 'select',
            description: 'DateCalendar variants',
            options: ['primary'],
        },
        size: {
            control: 'select',
            description: 'DateCalendar variants',
            options: ['sm', 'md', 'lg'],
        },
        endDateNumber: {
            control: 'number',
            description: 'Number of months for the date range',
        },
        onDateSelect: {
            action: 'clicked',
            description:
                'Function called when a date is selected. Receives a Date object.',
            table: {
                type: { summary: '(date: Date) => void' },
            },
        },
        className: {
            control: 'text',
            description:
                'Custom tailwind CSS classes to apply to the DateCalendar',
        },
        classNameText: {
            control: 'text',
            description:
                'Custom tailwind CSS classes to apply to the text in DateCalendar item',
        },
        language: {
            control: 'text',
            description: 'DateCalendar language',
        },
        sliderSettings: {
            control: 'object',
            description: 'Configuration settings for the slider.',
            table: {
                type: { summary: 'object' },
                defaultValue: {
                    summary: JSON.stringify(
                        {
                            speed: 300,
                            slidesToShow: 7,
                            slidesToScroll: 7,
                            infinite: false,
                            arrows: false,
                            dots: false,
                        },
                        null,
                        2
                    ),
                },
            },
        },
    },
};

export default meta;
type Story = StoryObj<typeof DateCalendar>;

export const Primary: Story = {
    args: {
        variant: 'primary',
        language: 'en',
    },
    play: async ({ canvasElement }) => {
        const canvas = within(canvasElement);

        // Check if the calendar is rendered
        expect(await canvas.findByTestId('date-calendar')).toBeInTheDocument();

        // Simulate a user clicking on a date (e.g., 14th)
        const dateButton = await canvas.findByTestId('date-14Feb2025'); // Use the full date
        await userEvent.click(dateButton);

        // Verify if the selected date is updated
        expect(dateButton).toHaveClass('bg-[#2B5162]'); // Modify based on actual selected class
    },
};

//TODO Create some tests
