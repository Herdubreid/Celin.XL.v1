import commonjs from '@rollup/plugin-commonjs';
import { nodeResolve } from '@rollup/plugin-node-resolve';
import typescript from '@rollup/plugin-typescript';

export default {
    input: './main.ts',
    output: {
        dir: '../wwwroot/assets',
        format: "umd",
        name: 'lib',
    },
    plugins: [
        commonjs(),
        nodeResolve(),
        typescript()],
};
