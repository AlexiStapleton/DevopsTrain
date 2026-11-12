import {Component} from "react";
import {Formik, Field, Form, ErrorMessage} from "formik";
import * as Yup from "yup";

import AuthService from "../Services/auth.service";
import type { RegisterRequest } from "../Model/RegisterRequest";

type Props = {}

type State = {
    registerRequest: RegisterRequest,
    success: boolean,
    message: string,
}

export default class Register extends Component<Props, State> {
    constructor(props: Props) {
        super(props);

        this.handlerRegister = this.handlerRegister.bind(this);

        this.state = {
            registerRequest: {lastname : '',
                firstname: '',
                email: '',
                password: '',
                passwordconfirm: ''},
            success: false,
            message: "",
        }
    }
    
    
    
    validationSchema() {
        return Yup.object().shape({
            firstname: Yup.string().required("First name is required"),
            lastname: Yup.string().required("Last name is required"),
            email: Yup.string().email("Email is not valid").required("Email is required"),
            password: Yup.string().required("Password is required"),
            passwordconfirm: Yup.string().required("Password confirmation is required")
                .test(
                    "len",
                    "The password must be between 6 and 40 characters.",
                    (val: any) =>
                        val &&
                        val.toString().length >= 6 &&
                        val.toString().length <= 40
                )
        });
    }
    
    handlerRegister(formValues: RegisterRequest) {
        const registerRequest: RegisterRequest = formValues;
        
        this.setState({
            message: "",
            success: false,
        });
        
        AuthService.register(
            registerRequest
        ).then(
            response => {
                this.setState({
                    message: response.data.message,
                    success: true
                });
            },
            error => {
                const resMessage = 
                    (error.response && 
                        error.response.data &&
                        error.response.data.message) ||
                    error.message ||
                    error.toString();
                
                this.setState({
                    success: false,
                    message: resMessage
                });
            }
        );
    }

    render() {
        const { success, message } = this.state;

        const initialValues = {
            firstname: "",
            lastname: "",
            email: "",
            password: "",
            passwordconfirm: "",
        };

        return (
            <div className="col-md-12">
                <div className="card card-container">
                    <img
                        src="//ssl.gstatic.com/accounts/ui/avatar_2x.png"
                        alt="profile-img"
                        className="profile-img-card"
                    />

                    <Formik
                        initialValues={initialValues}
                        validationSchema={this.validationSchema}
                        onSubmit={this.handlerRegister}
                    >
                        <Form>
                            {!success && (
                                <div>
                                    <div className="form-group">
                                        <label htmlFor="firstname"> FirstName </label>
                                        <Field name="firstname" type="text" className="form-control" />
                                        <ErrorMessage
                                            name="firstname"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="lastname"> LasteName </label>
                                        <Field name="lastname" type="text" className="form-control" />
                                        <ErrorMessage
                                            name="lastname"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="email"> Email </label>
                                        <Field name="email" type="email" className="form-control" />
                                        <ErrorMessage
                                            name="email"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="password"> Password </label>
                                        <Field
                                            name="password"
                                            type="password"
                                            className="form-control"
                                        />
                                        <ErrorMessage
                                            name="password"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>

                                    <div className="form-group">
                                        <label htmlFor="passwordconfirm"> Password Confirm </label>
                                        <Field
                                            name="passwordconfirm"
                                            type="password"
                                            className="form-control"
                                        />
                                        <ErrorMessage
                                            name="passwordconfirm"
                                            component="div"
                                            className="alert alert-danger"
                                        />
                                    </div>

                                    <div className="form-group">
                                        <button type="submit" className="btn btn-primary btn-block">Sign Up</button>
                                    </div>
                                </div>
                            )}

                            {message && (
                                <div className="form-group">
                                    <div
                                        className={
                                            success ? "alert alert-success" : "alert alert-danger"
                                        }
                                        role="alert"
                                    >
                                        {message}
                                    </div>
                                </div>
                            )}
                        </Form>
                    </Formik>
                </div>
            </div>
        );
    }
}