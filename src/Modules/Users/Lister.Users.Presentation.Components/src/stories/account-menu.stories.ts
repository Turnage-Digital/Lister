import { AccountMenu } from "../components";

import type { Meta, StoryObj } from "@storybook/react";

const meta = {
  title: "AccountMenu",
  component: AccountMenu,
} satisfies Meta<typeof AccountMenu>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {};
