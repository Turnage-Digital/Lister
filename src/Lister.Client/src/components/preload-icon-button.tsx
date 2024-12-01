import * as React from "react";

import { IconButton, IconButtonProps } from "@mui/material";
import { createLink, LinkComponent } from "@tanstack/react-router";

const MUILinkComponent = React.forwardRef<HTMLAnchorElement, IconButtonProps>(
  (props, ref) => {
    return <IconButton component="a" ref={ref} {...props} />;
  },
);
MUILinkComponent.displayName = "MUILinkComponent";

const CreatedLinkComponent = createLink(MUILinkComponent);

const PreloadIconButton: LinkComponent<typeof MUILinkComponent> = (props) => {
  return <CreatedLinkComponent preload="intent" {...props} />;
};

export default PreloadIconButton;
