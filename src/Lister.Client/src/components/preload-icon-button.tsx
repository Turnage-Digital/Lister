import * as React from "react";

import {IconButton, IconButtonProps} from "@mui/material";
import {createLink} from "@tanstack/react-router";

const MUILinkComponent = React.forwardRef<HTMLAnchorElement, IconButtonProps>(
    (props, ref) => {
        return <IconButton component="a" ref={ref} {...props} />;
    },
);
MUILinkComponent.displayName = "MUILinkComponent";

const PreloadIconButton = createLink(MUILinkComponent);

export default PreloadIconButton;
