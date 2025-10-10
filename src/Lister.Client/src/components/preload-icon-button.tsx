import * as React from "react";

import { IconButton, IconButtonProps } from "@mui/material";
import { Link, LinkProps } from "react-router-dom";

type PreloadIconButtonProps = IconButtonProps & {
  to: LinkProps["to"];
  replace?: LinkProps["replace"];
  state?: LinkProps["state"];
};

const PreloadIconButton = React.forwardRef<
  HTMLAnchorElement,
  PreloadIconButtonProps
>(function PreloadIconButton({ to, replace, state, ...iconButtonProps }, ref) {
  return (
    <IconButton
      component={Link}
      to={to}
      replace={replace}
      state={state}
      ref={ref}
      {...iconButtonProps}
    />
  );
});

PreloadIconButton.displayName = "PreloadIconButton";

export default PreloadIconButton;
