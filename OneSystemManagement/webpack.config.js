'use strict';

var path = require('path');
var webpack = require('webpack');
const CKEditorWebpackPlugin = require('@ckeditor/ckeditor5-dev-webpack-plugin');

const { bundler } = require('@ckeditor/ckeditor5-dev-utils');

const BabelMinifyPlugin = require('babel-minify-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = function (env) {

    env = env || {};
    var isProd = env.NODE_ENV === 'production';

    // Setup base config for all environments
    var config = {
        entry: {
            main: './wwwroot/js/site'
        },
        output: {
            path: path.join(__dirname, 'wwwroot/dist'),
            filename: '[name].js'
        },
        devtool: 'eval-source-map',
        resolve: {
            extensions: ['.ts', '.tsx', '.js', '.jsx'],
            modules: [
                path.resolve(__dirname, 'packages'),
                'node_modules'
            ]
        },
        plugins: [
            new CKEditorWebpackPlugin({
                languages: ['en']
            }),
            new ExtractTextPlugin('site.css'),
            new BabelMinifyPlugin(null, {
                comments: false
            }),
            new webpack.BannerPlugin({
                banner: bundler.getLicenseBanner(),
                raw: true
            }),
            new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery', 'window.jQuery': 'jquery' })
        ],
        module: {
            rules: [
                {
                    test: /\.svg$/,
                    use: ['raw-loader']
                },
                {
                    test: /\.scss$/,
                    use: ExtractTextPlugin.extract({
                        fallback: 'style-loader',
                        use: [
                            {
                                loader: 'css-loader',
                                options: {
                                    minimize: true
                                }
                            },
                            'sass-loader'
                        ]
                    })
                },
                {
                    test: require.resolve('jquery'),
                    use: [{
                        loader: 'expose-loader',
                        options: 'jQuery'
                    }, {
                        loader: 'expose-loader',
                        options: '$'
                    }]
                },
                { test: /\.css?$/, use: ['style-loader', 'css-loader'] },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' },
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=100000' }
            ]
        }
    }

    // Alter config for prod environment
    if (isProd) {
        config.devtool = 'source-map';
        config.plugins = config.plugins.concat([
            new webpack.optimize.UglifyJsPlugin({
                sourceMap: true
            })
        ]);
    }

    return config;
};