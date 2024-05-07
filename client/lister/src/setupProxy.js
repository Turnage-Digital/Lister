const { env } = require("process");

const { createProxyMiddleware } = require("http-proxy-middleware");

const target = env.ASPNETCORE_URLS.split(";")[0];
const context = ["/api"];

const onError = (err) => {
  console.error(`${err.message}`);
};

module.exports = function (app) {
  const appProxy = createProxyMiddleware(context, {
    target,
    onError,
    secure: false,
    headers: {
      Connection: "Keep-Alive",
    },
  });

  console.log(`Proxying requests to ${target} for ${context}`);
  app.use(appProxy);
};
