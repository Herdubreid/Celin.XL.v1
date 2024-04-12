const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CssMinimizerPlugin = require("css-minimizer-webpack-plugin");
const CustomFunctionsMetadataPlugin = require("custom-functions-metadata-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const sveltePreprocess = require("svelte-preprocess");

const mode = process.env.NODE_ENV || 'development';
const prod = mode === 'production';

module.exports = {
  mode: mode,
  entry: {
    "main": "./JS/main.ts",
    "login": "./JS/login.ts",
  },
  devtool: false,
  plugins: [
    new MiniCssExtractPlugin({
      filename: "[name].css"
    }),
    new CustomFunctionsMetadataPlugin({
      output: "functions.json",
      input: "./JS/functions.ts",
    }),
  ],
  optimization: {
    minimize: prod,
    minimizer: [
      new TerserPlugin(),
      new CssMinimizerPlugin(),
    ],
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        exclude: /node_modules/,
        use: {
          loader: "babel-loader",
          options: {
            presets: ["@babel/preset-env", "@babel/preset-typescript"],
          },
        },
      },
      {
        test: /\.svelte$/,
        use: {
          loader: 'svelte-loader',
          options: {
            compilerOptions: {
              dev: !prod
            },
            emitCss: prod,
            hotReload: !prod,
            preprocess: sveltePreprocess()
          }
        }
      },
      {
        test: /\.css$/,
        use: [
          {
            loader: MiniCssExtractPlugin.loader
          },
          "css-loader",
          "postcss-loader"
        ]
      },
      {
        test: /\.(woff|woff2|eot|ttf|otf)$/,
        use: [
          "file-loader"
        ]
      }
    ],
  },
  resolve: {
    alias: {
      svelte: path.resolve("node_modules", "svelte"),
    },
    extensions: [".mjs", ".ts", ".js", ".svelte"],
    mainFields: ["svelte", "browser", "module", "main"],
    conditionNames: ['svelte'],
  },
  output: {
    path: path.resolve(__dirname, "wwwroot/assets"),
    filename: "[name].js",
    library: "lib"
  },
};
