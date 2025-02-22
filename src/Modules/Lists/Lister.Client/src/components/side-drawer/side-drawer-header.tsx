import * as React from "react";

import {Close} from "@mui/icons-material";
import {Box, IconButton, Typography} from "@mui/material";

import useSideDrawer from "./use-side-drawer";

const SideDrawerHeader = () => {
    const {title, closeDrawer} = useSideDrawer();

    return (
        <Box
            sx={(theme) => ({
                display: "flex",
                alignItems: "center",
                padding: theme.spacing(2),
                ...theme.mixins.toolbar,
            })}
        >
            <Typography variant="h6" sx={{flexGrow: 1}}>
                {title}
            </Typography>

            <IconButton onClick={closeDrawer}>
                <Close/>
            </IconButton>
        </Box>
    );
};

export default SideDrawerHeader;
