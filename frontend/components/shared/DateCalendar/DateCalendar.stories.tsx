import type { Meta, StoryObj } from '@storybook/react';
import DateCalendar from './DateCalendar';

const meta: Meta<typeof DateCalendar> = {
    title: 'Components/shared/DateCalendar',
    component: DateCalendar,
    tags: ['autodocs'],
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
        language: 'pl',
    },
};
