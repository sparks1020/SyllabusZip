//Automation of tests while user is logged out of SyllabusZip

describe('Logged Out User Tests', () => {
    beforeEach(() => {
        cy.visit('https://syllabuszip.com')
    })
    //TC-21: LoggedoutSyllabusZip_ClicksLogin_LoggedIntoApplication
    it('Logs into application', () => {
  
      cy.contains('Login').click()

      // Should be on a new URL which includes '/commands/actions'
    cy.url().should('include', '/Account/Login')

    // Get an input, type into it and verify that the value has been updated
    cy.get('.form-control[type= email]')
      .type(Cypress.env('username'))

    cy.get('.form-control[type= password]')
      .type(Cypress.env('password'))
    
    cy.get('.btn.btn-primary[type=submit]').click()

    cy.get('.nav-link.dropdown-toggle')
       .should('contain', 'My Account')

    cy.Logout()
    })

  //TC-24: UseronLoggedOutHomepage_ClicksGetMoreDetails_BroughttoTechnologyPage
    it('Navigates to About the Technology Page', () => {
    
      cy.get('a[href*="/Home/AboutTheTechnology"]').click()

      cy.url().should('include', '/Home/AboutTheTechnology')

    
    })

  //TC-25: UseronLoggedOutHomepage_ClicksFindOutMore_BroughtTargetAudiencePage
    it('Navigates to the Target Audience Page', () => {

        cy.get('a[href*="/Home/Students"]').click()

        cy.url().should('include', '/Home/Students')

    })

  //TC-26: UseronLoggedOutHomepage_ClicksPricingButton_BroughtPricingPage
   it('Navigates to the Pricing Details Page', () => {

    cy.get('a[href*="/Home/Pricing"]').click()

    cy.url().should('include', '/Home/Pricing')

})

})