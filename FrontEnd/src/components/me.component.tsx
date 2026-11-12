import { Component } from "react";
import { Navigate } from "react-router-dom";
import AuthService from "../Services/auth.service";

type Props = {};

type State = {
    redirect: string | null;
    userReady: boolean;
    claims: { userId: string; exp: number; lastname: string; firstname: string } | null;
    token: string;
};

export default class Me extends Component<Props, State> {
    state: State = { redirect: null, userReady: false, claims: null, token: "" };

    componentDidMount() {
        const claims = AuthService.getCurrentUser();
        if (!claims) {
            this.setState({ redirect: "/login" });
            return;
        }
        this.setState({
            claims,
            token: AuthService.getToken() ?? "",
            userReady: true,
        });
    }

    render() {
        if (this.state.redirect) return <Navigate to={this.state.redirect} />;
        const { claims, token, userReady } = this.state;
        if (!userReady || !claims) return null;

        return (
            <div className="container">
                <header className="jumbotron">
                    <h3>Profile</h3>
                </header>
                <p><strong>Token :</strong> {token.substring(0, 20)} ... {token.slice(-20)}</p>
                <p><strong>Last Name :</strong> {claims.lastname}</p>
                <p><strong>First Name :</strong> {claims.firstname}</p>
                <p><strong>Id :</strong> {claims.userId}</p>
                <p><strong>Expire le :</strong> {new Date(claims.exp * 1000).toLocaleString()}</p>
            </div>
        );
    }
}