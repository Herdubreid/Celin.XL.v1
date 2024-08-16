import commonjs from '@rollup/plugin-commonjs';
import typescript from '@rollup/plugin-typescript';
import terser from '@rollup/plugin-terser'
import { nodeResolve } from '@rollup/plugin-node-resolve';
import svelte from 'rollup-plugin-svelte';
import postcss from 'rollup-plugin-postcss';
import tailwindcss from 'tailwindcss';
import autoprefixer from 'autoprefixer';
import { sveltePreprocess } from 'svelte-preprocess';

export default [
    {
        input: './main.ts',
        output: {
            dir: '../wwwroot/assets',
            format: 'umd',
            name: 'lib',
            sourcemap: true,
            plugins: [terser()],
        },
        plugins: [
            commonjs(),
            nodeResolve(),
            typescript({ sourceMap: true })],
    },
    {
        input: './login.ts',
        output: {
            dir: '../wwwroot/assets',
            sourcemap: true,
            format: 'iife',
            name: 'app',
            sourcemap: true,
            plugins: [terser()],
        },
        plugins: [
            commonjs(),
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
            }),
            typescript({ sourceMap: true })],
    },
    {
        input: './editor.ts',
        output: {
            dir: '../wwwroot/assets',
            sourcemap: true,
            format: 'iife',
            name: 'app',
            sourcemap: true,
            plugins: [terser()],
        },
        plugins: [
            commonjs(),
            nodeResolve({
                browser: true,
                exportConditions: ['svelte'],
                extensions: ['.svelte']
            }),
            svelte({
                preprocess: sveltePreprocess()
            }),
            postcss({
                plugins: [
                    tailwindcss(),
                    autoprefixer(),
                ],
                extract: true,
            }),
            typescript({ sourceMap: true })],
    }
];
