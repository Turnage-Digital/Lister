import type { Preview } from "@storybook/react";

const link = document.createElement("link");
link.rel = "stylesheet";
link.href = "https://fonts.googleapis.com/icon?family=Material+Icons";
document.head.appendChild(link);

const preview: Preview = {
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
  },
};

export default preview;
