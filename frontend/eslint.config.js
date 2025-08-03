import { dirname } from 'path';
import { fileURLToPath } from 'url';
import { FlatCompat } from '@eslint/eslintrc';
import preferArrow from 'eslint-plugin-prefer-arrow';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const compat = new FlatCompat({
    baseDirectory: __dirname,
});

const eslintConfig = [
    {
        ignores: ['components/ui/**'],
    },
    ...compat.extends('next/core-web-vitals', 'next/typescript'),
    {
        rules: {
            'prefer-arrow/prefer-arrow-functions': 'error',
        },
        plugins: {
            'prefer-arrow': preferArrow,
        },
    },
];

export default eslintConfig;
