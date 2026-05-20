---
name: JPay Marketing System
colors:
  surface: '#f8f9fc'
  surface-dim: '#d9dadd'
  surface-bright: '#f8f9fc'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f2f3f6'
  surface-container: '#edeef1'
  surface-container-high: '#e7e8eb'
  surface-container-highest: '#e1e2e5'
  on-surface: '#191c1e'
  on-surface-variant: '#4f434c'
  inverse-surface: '#2e3133'
  inverse-on-surface: '#f0f1f4'
  outline: '#80737d'
  outline-variant: '#d2c2cd'
  surface-tint: '#844981'
  primary: '#300033'
  on-primary: '#ffffff'
  primary-container: '#4a154b'
  on-primary-container: '#be7db9'
  inverse-primary: '#f6afef'
  secondary: '#23667a'
  on-secondary: '#ffffff'
  secondary-container: '#a8e6fd'
  on-secondary-container: '#26697c'
  tertiary: '#001b17'
  on-tertiary: '#ffffff'
  tertiary-container: '#00322c'
  on-tertiary-container: '#3da293'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffd6f8'
  primary-fixed-dim: '#f6afef'
  on-primary-fixed: '#370139'
  on-primary-fixed-variant: '#693168'
  secondary-fixed: '#b5ebff'
  secondary-fixed-dim: '#92d0e6'
  on-secondary-fixed: '#001f28'
  on-secondary-fixed-variant: '#004e60'
  tertiary-fixed: '#92f4e3'
  tertiary-fixed-dim: '#76d7c7'
  on-tertiary-fixed: '#00201c'
  on-tertiary-fixed-variant: '#005047'
  background: '#f8f9fc'
  on-background: '#191c1e'
  surface-variant: '#e1e2e5'
typography:
  display-lg:
    fontFamily: Manrope
    fontSize: 48px
    fontWeight: '700'
    lineHeight: '1.1'
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Manrope
    fontSize: 32px
    fontWeight: '700'
    lineHeight: '1.2'
  headline-lg-mobile:
    fontFamily: Manrope
    fontSize: 24px
    fontWeight: '700'
    lineHeight: '1.2'
  headline-md:
    fontFamily: Manrope
    fontSize: 24px
    fontWeight: '600'
    lineHeight: '1.3'
  body-lg:
    fontFamily: Hanken Grotesk
    fontSize: 18px
    fontWeight: '400'
    lineHeight: '1.6'
  body-md:
    fontFamily: Hanken Grotesk
    fontSize: 16px
    fontWeight: '400'
    lineHeight: '1.5'
  label-sm:
    fontFamily: Hanken Grotesk
    fontSize: 12px
    fontWeight: '600'
    lineHeight: '1'
    letterSpacing: 0.05em
  numeric-data:
    fontFamily: Manrope
    fontSize: 20px
    fontWeight: '700'
    lineHeight: '1'
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  container-max: 1280px
  gutter: 24px
  margin-desktop: 64px
  margin-mobile: 20px
  stack-sm: 8px
  stack-md: 16px
  stack-lg: 32px
---

## Brand & Style
The brand personality is efficient, approachable, and modern. It targets small-to-medium business owners who value simplicity and clarity in their operations. The visual identity is built on a "Functional Sophistication" narrative—balancing a deep, authoritative brand purple with clean, expansive white surfaces that suggest room to grow.

The design style is **Corporate / Modern** with a focus on high-clarity data visualization. It uses structured grid layouts and subtle depth to make complex information (like analytics and transaction logs) feel manageable and intuitive. The interface should evoke a sense of organized calm and professional reliability.

## Colors
The palette is led by a **Deep Plum Purple** (#4A154B) used for high-impact brand moments, sidebars, and primary calls to action. A **Deep Teal** (#005366) serves as the secondary color, primarily for data visualization and success-oriented metrics. A **Soft Mint** (#63C5B5) is the tertiary accent, used for "on-going" statuses and secondary growth indicators.

The background system relies on a very light **Ice Blue Neutral** (#F8F9FC) to create a soft contrast against pure white cards, reducing eye strain during long-term administrative use. Text should primarily use a dark grey for body copy to maintain readability, while headers adopt the primary purple.

## Typography
The system uses a dual-font strategy to balance character and utility. **Manrope** is the headline face, providing a geometric yet friendly structure that works exceptionally well for large sales figures and marketing headers. **Hanken Grotesk** is the workhorse for body copy and UI labels, chosen for its high legibility in dense data tables and technical descriptions.

For marketing layouts, use `display-lg` for hero sections. In the dashboard and POS views, `numeric-data` should be used for currency and transaction totals to ensure they stand out as the most critical information on the screen.

## Layout & Spacing
This design system uses a **Fluid Grid** model with a maximum container width of 1280px for desktop. The marketing page follows a 12-column grid, while the internal application dashboard uses a 240px fixed sidebar with a fluid content area.

Spacing is based on an 8px base unit. Gutters are set to 24px to provide significant "breathing room" between content blocks, reinforcing the clean and organized brand feel. On mobile devices, margins shrink to 20px, and complex 3-column feature grids reflow into a single vertical stack. Section-to-section vertical spacing should be generous (80px - 120px) to delineate different product features like "Promotion Management" vs "Dashboard Analytics."

## Elevation & Depth
Depth is achieved through **Tonal Layers** and extremely soft **Ambient Shadows**. The primary background is the neutral off-white, and functional containers (cards, tables, and the POS cart) are pure white. 

Shadows should be barely perceptible—using a large blur radius (20px+) with low opacity (4-6%) and a slight tint of the primary purple to keep them from looking "dirty." This creates a "lifted card" effect that helps users distinguish interactive areas from the static background. For the POS interface specifically, the "Charge" button and active order items use a slightly higher elevation to draw immediate focus to the action of completing a sale.

## Shapes
The shape language is consistently **Rounded**, using 0.5rem (8px) for standard UI elements like input fields and small buttons. Larger containers, such as product cards and dashboard modules, utilize the `rounded-lg` (16px) or `rounded-xl` (24px) settings to soften the overall aesthetic. 

Search bars and status badges (like "On-Going" or "Active") should utilize the pill-shaped max rounding to distinguish them from structural layout blocks.

## Components
- **Buttons:** Primary buttons are solid Purple with white text. Secondary buttons use the Deep Teal. Ghost buttons use purple text with no fill. All buttons have a hover state that darkens the fill by 10%.
- **Input Fields:** Use a subtle light-grey background (#F1F3F9) with no border until focused. Upon focus, they should have a 1px border of the primary purple.
- **Chips / Badges:** Used for category filters (e.g., "Coffee", "Pastry"). Active chips are solid purple; inactive chips are light grey with dark text.
- **Data Tables:** Clean, borderless rows with 1px horizontal dividers. The header row should be in the `label-sm` style with a subtle grey background.
- **Cards:** Product cards in the POS feature large imagery with prices in the primary purple at the bottom.
- **Analytics Widgets:** Circular progress bars for "Target Monthly Sales" use the Teal/Mint accent colors to provide a clear visual of completion.