import * as React from "react";
import {ReactNode} from "react";

import {Grid2, Stack, Typography} from "@mui/material";

interface Props {
    title: string;
    blurb: string;
    content: ReactNode;
}

const FormBlock = ({title, blurb, content}: Props) => {
    return (
        <Grid2 container>
            <Grid2 size={{xs: 12, md: 4}}>
                <Stack spacing={2} sx={{pb: {xs: 4, md: 0}}}>
                    <Typography color="primary" fontWeight="medium" variant="h6">
                        {title}
                    </Typography>
                    <Typography
                        variant="body1"
                        sx={{color: "text.secondary"}}
                        gutterBottom
                    >
                        {blurb}
                    </Typography>
                </Stack>
            </Grid2>
            <Grid2 size={{xs: 12, md: 4}}>{content}</Grid2>
        </Grid2>
    );
};

export default FormBlock;
