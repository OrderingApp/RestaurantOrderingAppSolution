import { Meta, StoryObj } from '@storybook/react';
import Input from './Input';

const meta: Meta<typeof Input> = {
    title: 'Components/shared/Input',
    component: Input,
    tags: ['autodocs'],
    parameters: {
        docs: {
            description: {
                component:
                    'A customizable input component with support for different sizes, variants, error handling, and accessibility features.',
            },
        },
    },
    argTypes: {
        type: {
            control: 'text',
            description:
                'Specifies the type of input (e.g., text, password, email).',
        },
        label: {
            control: 'text',
            description: 'Label text displayed above the input field.',
        },
        placeholder: {
            control: 'text',
            description: 'Placeholder text displayed inside the input field.',
        },
        inputSize: {
            control: { type: 'radio' },
            options: ['sm', 'md', 'lg'],
            description: 'Determines the size of the input field.',
        },
        variant: {
            control: { type: 'radio' },
            options: ['primary'],
            description: 'Defines the visual styling of the input field.',
        },
        id: {
            control: 'text',
            description:
                'A unique identifier for the input field, used for accessibility and linking the label to the input.',
        },
        autoCapitalize: {
            control: 'text',
            description:
                'Controls text capitalization behavior (e.g., "none", "sentences", "words", "characters").',
        },
        autoCorrect: {
            control: 'text',
            description:
                'Enables or disables automatic text correction (e.g., "on" or "off").',
        },
        labelClassName: {
            control: 'text',
            description: 'Custom tailwind CSS classes to apply to the label',
        },

        inputClassName: {
            control: 'text',
            description: 'Custom tailwind CSS classes to apply to the input',
        },

        errorClassName: {
            control: 'text',
            description: 'Custom tailwind CSS classes to apply to the error',
        },

        disabled: {
            control: 'boolean',
            description: 'Disables the input field, making it uneditable.',
        },
        onChange: {
            action: 'changed',
            description:
                'Function that is called when the input value changes. Receives the event object as an argument.',
        },
        errors: {
            control: 'object',
            description: 'Displays an error message when validation fails.',
        },
        props: {
            control: 'object',
            description: 'Additional input properties.',
        },
    },
};

export default meta;

type Story = StoryObj<typeof Input>;

export const Default: Story = {
    args: {
        type: 'text',
        label: 'Default Input',
        placeholder: 'Enter text...',
        inputSize: 'sm',
        variant: 'primary',
    },
    parameters: {
        docs: {
            description: {
                story: 'This is the standard input field with default settings.',
            },
        },
    },
};

export const WithError: Story = {
    args: {
        type: 'text',
        label: 'Input with Error',
        placeholder: 'Enter text...',
        inputSize: 'sm',
        variant: 'primary',
        errors: { message: 'This field is required', type: 'Required' },
    },
    parameters: {
        docs: {
            description: {
                story: 'Displays an error message when the input validation fails.',
            },
        },
    },
};

export const Disabled: Story = {
    args: {
        type: 'text',
        label: 'Disabled Input',
        placeholder: 'Cannot type here...',
        inputSize: 'sm',
        variant: 'primary',
        disabled: true,
    },
    parameters: {
        docs: {
            description: {
                story: 'This input field is disabled, preventing any user interaction.',
            },
        },
    },
};

//TODO Add some tests
