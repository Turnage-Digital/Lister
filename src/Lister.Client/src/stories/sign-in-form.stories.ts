import { SignInForm } from "../components";

import type { Meta, StoryObj } from "@storybook/react";

const meta = {
  title: "SignInForm",
  component: SignInForm
} satisfies Meta<typeof SignInForm>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    onSubmit: () => {
    }
  }
};
