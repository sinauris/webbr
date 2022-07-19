const path = require('path');
const webpack = require('webpack');
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = env => {
    const isDevBuild = !(env && env.production);

    return [{
        mode: isDevBuild ? "development" : "production",
        stats: { modules: false },
        entry: { main: './ClientApp/main.js' },
        resolve: {
            extensions: ['.js', '.vue'],
            alias: {
                'vue$': 'vue/dist/vue',
                'components': path.resolve(__dirname, './ClientApp/components')
            }
        },
        output: {
            path: path.join(__dirname, 'wwwroot', 'dist'),
            publicPath: '/dist/',
            filename: '[name].js'
        },
        module: {
            rules: [
                { test: /\.vue$/, include: /ClientApp/, use: 'vue-loader' },
                { test: /\.js$/, include: /ClientApp/, use: 'babel-loader' },
                
                
                
                { test: /\.css(\?|$)/, use: isDevBuild ? ['style-loader', 'css-loader'] : [{ loader: MiniCssExtractPlugin.loader }, "css-loader"] },
                { test: /\.scss/, use: ['vue-style-loader', 'css-loader', 'sass-loader'] },
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=25000' }
            ],
        },
        plugins: [
            new VueLoaderPlugin(),
            new MiniCssExtractPlugin({ filename: '[name].css', chunkFilename: '[id].css' }),
            new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery', 'window.jQuery': 'jquery' })
        ]
    }];
};
