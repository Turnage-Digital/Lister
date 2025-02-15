import * as React from "react";

import {Stack, Typography} from "@mui/material";
import {createFileRoute, useRouter} from "@tanstack/react-router";

import {SignInForm} from "../components";

const RouteComponent = () => {
    const router = useRouter();
    const navigate = Route.useNavigate();
    const search = Route.useSearch();
    const {auth, status} = Route.useRouteContext({
        select: ({auth}) => ({auth, status: auth.status}),
    });
    // const [showForgotPassword, setShowForgotPassword] = React.useState(false);

    React.useLayoutEffect(() => {
        if (status === "loggedIn" && search.callbackUrl) {
            router.history.push(search.callbackUrl);
        }
    }, [status, search.callbackUrl, router.history]);

    // const handleForgotPasswordClick = () => {
    //   setShowForgotPassword(true);
    // };
    //
    // const handleForgotPasswordClose = () => {
    //   setShowForgotPassword(false);
    // };

    const handleSignedIn = async (email: string) => {
        auth.login(email);
        await navigate({to: search.callbackUrl});
    };

    return (
        <Stack sx={{width: "450px", mx: "auto", px: 2, py: 4}} spacing={4}>
            <Typography variant="h5">Sign in</Typography>

            <SignInForm onSignedIn={handleSignedIn}/>

            {/* <ForgotPasswordDialog*/}
            {/*  open={showForgotPassword}*/}
            {/*  onClose={handleForgotPasswordClose}*/}
            {/*  onSubmit={(_) => {*/}
            {/*    // console.log("forgot password", email);*/}
            {/*  }}*/}
            {/* />*/}

            {/* <Typography variant="body1">*/}
            {/*  <Link*/}
            {/*    component="button"*/}
            {/*    type="button"*/}
            {/*    onClick={handleForgotPasswordClick}*/}
            {/*  >*/}
            {/*    Forgot your password?*/}
            {/*  </Link>*/}
            {/* </Typography>*/}

            {/* <Typography variant="body1">*/}
            {/*  Don&apos;t have an account?{" "}*/}
            {/*  <Link component="button" type="button">*/}
            {/*    Sign up*/}
            {/*  </Link>*/}
            {/* </Typography>*/}
        </Stack>
    );
};

export interface SignInSearch {
    callbackUrl?: string;
}

export const Route = createFileRoute("/sign-in")({
    component: RouteComponent,
    validateSearch: (search): SignInSearch => {
        return {
            callbackUrl: search.callbackUrl as string | undefined,
        };
    },
});
