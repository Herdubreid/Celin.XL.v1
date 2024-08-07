import commonjs from '@rollup/plugin-commonjs';
import typescript from '@rollup/plugin-typescript';
import terser from '@rollup/plugin-terser'
import { nodeResolve } from '@rollup/plugin-node-resolve';
import svelte from 'rollup-plugin-svelte';
import postcss from 'rollup-plugin-postcss';
import tailwindcss from 'tailwindcss';
import autoprefixer from 'autoprefixer';

export default [
    {
        input: './main.ts',
        output: {
            dir: '../wwwroot/assets',
            format: 'umd',
            name: 'lib',
            plugins: [terser()],
        },
        plugins: [
            commonjs(),
            nodeResolve(),
            typescript()],
    },
    {
        input: './login.ts',
        output: {
            dir: '../wwwroot/assets',
            sourcemap: true,
            format: 'iife',
            name: 'app',
            plugins: [terser()],
        },
        plugins: [
            commonjs(),
            typescript(),
            nodeResolve({
                browser: true,
                exportConditions: ['svelte'],
                extensions: ['.svelte']
            }),
            svelte({
                preprocess: {
                    style: ({ content }) => {
                        return transformStyles(content);
                    }
                },
            }),
            postcss({
                plugins: [
                    tailwindcss(),
                    autoprefixer(),
                ],
                extract: true,
            })],
    }
];
