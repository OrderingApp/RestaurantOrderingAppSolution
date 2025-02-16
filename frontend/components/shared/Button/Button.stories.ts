import type { Meta, StoryObj } from '@storybook/react';
import Button from './Button';

const meta: Meta<typeof Button> = {
    title: 'Components/shared/button',
    component: Button,
    tags: ['autodocs'],
    parameters: {
        layout: 'centered',
        docs: {
            description: {
                component: 'A customizable Button component',
            },
        },
    },
    argTypes: {
        variant: {
            control: 'select',
            description: 'Button variants',
            options: ['primary', 'success', 'danger', 'outline'],
        },
        size: {
            control: 'select',
            description: 'Button sizes',
            options: ['sm', 'md', 'lg'],
        },
        onClick: {
            action: 'clicked',
            description: 'Function called when defualt button is clicked',
        },
        children: {
            control: 'text',
            description: 'Content to be displayed inside the button',
        },
        className: {
            control: 'text',
            description: 'Custom tailwind CSS classes to apply to the button',
        },
    },
};

export default meta;
type Story = StoryObj<typeof Button>;

export const Primary: Story = {
    args: {
        variant: 'primary',
        size: 'md',
        disabled: false,
        children: 'Primary Btn',
    },
};

export const Secondary: Story = {
    args: {
        variant: 'success',
        size: 'md',
        disabled: false,
        children: 'Success Btn',
    },
};

export const Danger: Story = {
    args: {
        variant: 'danger',
        size: 'md',
        disabled: false,
        children: 'Danger Btn',
    },
};

export const Outline: Story = {
    args: {
        variant: 'outline',
        size: 'md',
        disabled: false,
        children: 'Outline Btn',
    },
};
