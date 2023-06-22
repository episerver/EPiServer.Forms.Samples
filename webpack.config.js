const glob = require("glob");
const path = require("path");
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = (env, argv) => {
    return {
        entry: glob.sync("./src/**/index.tsx").reduce((acc, item) => {
            const path = item.split("/");
            path.pop();
            const name = path.pop();
            acc[name] = item;
            return acc;
        }, {}),
        output: {
            filename: "index.js",
            path: path.resolve(__dirname, "src/EPiServer.Forms.Samples/ClientResources/Criteria/dist/"),
            libraryTarget: "commonjs" // Must be CommonJS since the modules will be imported using https://github.com/Paciolan/remote-component
        },
        externals: {
            react: "react",
            reactDOM: 'react-dom',
            '@episerver/ui-framework': '@episerver/ui-framework',
            axios: "axios"
        },
        plugins: [
            new MiniCssExtractPlugin()
        ],
        module: {
            rules: [
                {
                    test: /\.tsx?$/,
                    exclude: /node_modules/,
                    use: [
                        {
                            loader: 'ts-loader',
                            options: {
                                transpileOnly: true
                            }
                        }
                    ]
                },
                {
                    test: /\.s?css$/,
                    use: [
                        {
                            loader: argv.mode === "production" ? MiniCssExtractPlugin.loader : 'style-loader',
                        },
                        {
                            loader: 'css-loader',
                        },
                        {
                            loader: 'sass-loader',
                            options: {
                                sassOptions: {
                                    includePaths: ['node_modules'],
                                }
                            }
                        }
                    ]
                }
            ]
        }
    }
}; 