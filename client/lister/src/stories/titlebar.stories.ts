import type { Meta, StoryObj } from "@storybook/react";

import { Action, Titlebar } from "../components";

const meta = {
  title: "Titlebar",
  component: Titlebar,
} satisfies Meta<typeof Titlebar>;

export default meta;
type Story = StoryObj<typeof meta>;

export const ListsPage: Story = {
  args: {
    title: "Lists",
    actions: [
      {
        title: "Create a List",
      },
    ],
  },
};

export const ListPage: Story = {
  args: {
    title: "List",
    actions: [
      {
        title: "Create an Item",
      },
    ],
    breadcrumbs: [
      {
        title: "Lists",
        url: "/",
      },
    ],
  },
};
