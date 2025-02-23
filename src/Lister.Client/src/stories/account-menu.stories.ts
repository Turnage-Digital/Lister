import { AccountMenu } from "../components/account-menu";

import type { Meta, StoryObj } from "@storybook/react";

const meta = {
  title: "AccountMenu",
  component: AccountMenu,
  parameters: {
    backgrounds: {
      default: "blue",
      values: [{ name: "blue", value: "#003E6B" }],
    },
  },
} satisfies Meta<typeof AccountMenu>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {};
