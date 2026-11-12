import { Component} from "react";
import { Navigate } from "react-router-dom";
import { Formik, Field, Form, ErrorMessage } from 'formik';

import * as Yup from "yup";

import AuthService from '../Services/auth.service';


type Props = {
    onLogin?: () => void
};

type State = {
    redirect: string | null,
    email: string,
    password: string,
    loading: boolean,
    message: string
};

export default class Login extends Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.handleLogin = this.handleLogin.bind(this);
        
        this.state = {
            redirect: null,
            email: "",
            password: "",
            loading: false,
            message: "",
        }
    }
    
    componentDidMount() {
        const currentUser = AuthService.getCurrentUser();
        
        if (currentUser) {
            this.setState({ redirect: "/me" })
        }
    }
    
    validationSchema() {
        return Yup.object().shape({
            email: Yup.string().required("Email is required"),
            password: Yup.string().required("Password is required"),
        })
    }
    
    handleLogin(formValue: { email: string, password: string }) {
        const {email, password} = formValue;
        
        this.setState({
            message:"",
            loading: true
        });
        
        AuthService.login(email, password).then(
            () => {
                this.props.onLogin?.();
                this.setState({ redirect: "/me" })
            },
            error => {
                const resMessage =
                    (error.response && error.response.data && error.response.data.message) ||
                    error.message ||
                    error.toString();
                
                this.setState({
                    loading: false,
                    message: resMessage
                });
            }
        )
    }
    
    render() {
        if (this.state.redirect) {
            return <Navigate to={this.state.redirect} />
        }
        
        const { loading, message } = this.state;
        
        const initialValues = {
            email: "",
            password: "",
        };
        
        return (

        <Formik
            initialValues={initialValues}
        validationSchema={this.validationSchema()}
        onSubmit={this.handleLogin}
        >
        <Form>
            <div className="form-group">
        <label htmlFor="email">Email</label>
            <Field name="email" type="text" className="form-control" />
        <ErrorMessage
            name="email"
        component="div"
        className="alert alert-danger"
            />
            </div>

            <div className="form-group">
        <label htmlFor="password">Password</label>
            <Field name="password" type="password" className="form-control" />
        <ErrorMessage
            name="password"
        component="div"
        className="alert alert-danger"
            />
            </div>

            <div className="form-group">
        <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading && (
                <span className="spinner-border spinner-border-sm"></span>
            )}
        <span>Login</span>
                            </button>
                        </div>

                        {message && (
                            <div className="form-group">
                            <div className="alert alert-danger" role="alert">
                            {message}
                            </div>
                            </div>
                        )}
                    </Form>
                </Formik>
        )
    }
    
}